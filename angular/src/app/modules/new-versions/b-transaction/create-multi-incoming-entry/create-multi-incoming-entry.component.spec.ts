import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMultiIncomingEntryComponent } from './create-multi-incoming-entry.component';

describe('CreateMultiIncomingEntryComponent', () => {
  let component: CreateMultiIncomingEntryComponent;
  let fixture: ComponentFixture<CreateMultiIncomingEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateMultiIncomingEntryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMultiIncomingEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
