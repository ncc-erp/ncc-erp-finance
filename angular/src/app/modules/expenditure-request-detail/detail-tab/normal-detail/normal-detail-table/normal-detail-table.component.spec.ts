import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NormalDetailTableComponent } from './normal-detail-table.component';

describe('NormalDetailTableComponent', () => {
  let component: NormalDetailTableComponent;
  let fixture: ComponentFixture<NormalDetailTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NormalDetailTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NormalDetailTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
