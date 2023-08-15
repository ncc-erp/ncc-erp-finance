import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkRevenuesComponent } from './link-revenues.component';

describe('LinkRevenuesComponent', () => {
  let component: LinkRevenuesComponent;
  let fixture: ComponentFixture<LinkRevenuesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkRevenuesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkRevenuesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
